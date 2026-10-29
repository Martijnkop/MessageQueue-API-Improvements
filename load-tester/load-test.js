import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:32770'

const WRITE_STATUS = (__ENV.WRITE_STATUS || '200,201').split(',').map(Number);
const TEST_TYPE = __ENV.TEST_TYPE || 'load'; // smoke | load
const VUS = parseInt(__ENV.VUS || '10', 10);
const DURATION = __ENV.DURATION || '10m';

const WRITE_HEAVY = __ENV.WRITE_HEAVY === 'true';

const P_SEARCH = num('P_SEARCH', 0.3);
const P_OPEN = num('P_OPEN', 0.6);
const P_LIKE = num('P_LIKE', WRITE_HEAVY ? 0.8 : 0.4);
const P_FOLLOW = num('P_FOLLOW', WRITE_HEAVY ? 0.4 : 0.15);
const P_CREATE = num('P_CREATE', WRITE_HEAVY ? 0.5 : 0.1);

const SLEEP_MIN = num('SLEEP_MIN', 1);
const SLEEP_MAX = num('SLEEP_MAX', 5);

function num(name, fallback) {
  return __ENV[name] !== undefined ? parseFloat(__ENV[name]) : fallback;
}

const ENDPOINTS = {
  feed: () => `${BASE_URL}/posts`,
  // search: (term) => `${BASE_URL}/posts/search?query=${encodeURIComponent(term)}`,
  getById: (id) => `${BASE_URL}/posts/${id}`,
  like: () => `${BASE_URL}/posts/like`,
  // follow: (userId) => `${BASE_URL}/api/users/${userId}/follow`,
  create: () => `${BASE_URL}/posts`,
};

// =========
// Test Data
// =========
const users = new SharedArray('users', () => JSON.parse(open('./users.json')));
const searchTerms = new SharedArray('searchTerms', () => JSON.parse(open('./searchterms.json')));

// =========
// OPTIONS
// =========
const scenarios = {
  smoke: {
    executor: 'per-vu-iterations',
    vus: 1,
    iterations: 5,
    maxDuration: '2m',
  },
  load: {
    // Vast aantal VU's gedurende de hele test (geen ramp-up)
    executor: 'constant-vus',
    vus: VUS,
    duration: DURATION,
  },
};

export const options = {
  scenarios: { [TEST_TYPE]: scenarios[TEST_TYPE] },
  thresholds: {
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
    http_req_failed: ['rate<0.01'],
    checks: ['rate>0.99'],
  },
};

// Startup check amount of users
export function setup() {
  const needed = TEST_TYPE === 'smoke' ? 1 : VUS;
  if (users.length < needed) {
    throw new Error(
      `users.json has ${users.length} / ${needed} required users`
    );
  }
}

// =======
// HELPERS
// =======
function tagged(params, name) {
  return Object.assign({}, params, { tags: { name } });
}

const DEBUG = __ENV.DEBUG === 'true';
function logIfUnexpected(label, res) {
  if (DEBUG && !WRITE_STATUS.includes(res.status)) {
    console.warn(`${label}: status ${res.status}, body: ${String(res.body).slice(0, 300)}`);
  }
}

function chance(p) {
  if (TEST_TYPE === "smoke") return true
  return Math.random() < p;
}

function pick(arr) {
  return arr[Math.floor(Math.random() * arr.length)];
}

function extractPosts(res) {
  try {
    const body = res.json();
    if (Array.isArray(body)) return body;
    for (const key of ['content', 'items', 'posts', 'data']) {
      if (Array.isArray(body[key])) return body[key];
    }
  } catch (e) {
    // invalid json
  }
  return [];
}

function authorIdOf(post) {
  return post.authorId ?? post.userId ?? (post.author && post.author.id);
}

// ====
// FLOW
// ====
export default function () {
  const user = users[(__VU - 1) % users.length]
  const params = {
    headers: {
      "Authorization": `Bearer ${user.accessToken}`,
      "Content-Type": "application/json"
    }
  }

  // 0. Sleep
  if (__ITER === 0) {
    sleep(Math.random() * (SLEEP_MAX - SLEEP_MIN));
  }

  // 1. Load feed
  const feedResult = http.get(ENDPOINTS.feed(), tagged(params, 'GetFeed'));
  check(feedResult, { "feed: status 200": (r) => r.status === 200 });
  let candidates = extractPosts(feedResult)

  // 2. Search
  // if (chance(P_SEARCH)) {
  //     const searchRes = http.get(ENDPOINTS.search(pick(searchTerms)), params, {
  //       tags: { name: 'SearchFeed' },
  //     });
  //     check(searchRes, { 'search: status 200': (r) => r.status === 200 });
  //     const found = extractPosts(searchRes);
  //     if (found.length > 0) candidates = found; // Open from search or feed
  //   }

  // Search is commented out because its not implemented, so we open a random post from feed

  // 3. Open post
  if (candidates.length > 0 && chance(P_OPEN)) {
    const post = pick(candidates);

    const openRes = http.get(ENDPOINTS.getById(post.id), tagged(params, 'GetPostById'));
    check(openRes, { 'get by id: status 200': (r) => r.status === 200 });

    // 3a. Like
    if (chance(P_LIKE)) {
      const likeRes = http.post(ENDPOINTS.like(), JSON.stringify(post.id), tagged(params, 'LikePost'));
      logIfUnexpected('like', likeRes);
      check(likeRes, { 'like: verwachte status': (r) => WRITE_STATUS.includes(r.status) });
    }

    // 3b. Follow author
    // const authorId = authorIdOf(post);
    // if (authorId !== undefined && chance(P_FOLLOW)) {
    //   const followRes = http.post(ENDPOINTS.follow(authorId), null, params, {
    //     tags: { name: 'FollowUser' },
    //   });
    //   check(followRes, { 'follow: verwachte status': (r) => WRITE_STATUS.includes(r.status) });
    // }
  }

  // 4. Create new post
  if (chance(P_CREATE)) {
    const payload = JSON.stringify({
      message: `K6 testpost van VU ${__VU}, iteratie ${__ITER}, ${Date.now()}`,
      title: `K6 testpost van VU ${__VU}, iteratie ${__ITER}, ${Date.now()}`,
    });
    const createRes = http.post(ENDPOINTS.create(), payload, tagged(params, 'CreatePost'));
    logIfUnexpected('create', createRes);
    check(createRes, { 'create: verwachte status': (r) => WRITE_STATUS.includes(r.status) });
  }

  // 5. Waiting
  sleep(SLEEP_MIN + Math.random() * (SLEEP_MAX - SLEEP_MIN));
}
