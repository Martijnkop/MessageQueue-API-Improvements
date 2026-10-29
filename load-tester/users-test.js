import http from 'k6/http';
import { check, sleep } from 'k6';

// Override from the CLI: k6 run -e VUS=25 -e DURATION=30m users-test.js
const VUS = parseInt(__ENV.VUS || '10');
const DURATION = __ENV.DURATION || '1m';

export const options = {
  scenarios: {
    fixed_users: {
      executor: 'constant-vus', // fixed number of concurrent virtual users
      vus: VUS,
      duration: DURATION,
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
  },
};

export default function () {
  // Spread the VUs out on the first iteration so they don't all fire at once
  if (__ITER === 0) {
    sleep(Math.random() * 6);
  }

  const res = http.get('http://localhost:5015/users');

  check(res, {
    'status is 200': (r) => r.status === 200,
  });

  // Wait a random 6-12 seconds before the next request
  sleep(6 + Math.random() * 6);
}
