export function createProgressLogger(
  label: string,
  target: number,
  stepPercent = 10,
): (current: number) => void {
  let lastBucket = -1

  return (current: number) => {
    const percent = Math.min(100, Math.floor((current / target) * 100))
    const bucket = percent === 100 ? 100 : Math.floor(percent / stepPercent)
    if (bucket === lastBucket) return
    lastBucket = bucket

    const width = 20
    const filled = Math.round((percent / 100) * width)
    const bar = "#".repeat(filled) + ".".repeat(width - filled)
    console.log(
      `${label.padEnd(6)} [${bar}] ${String(percent).padStart(3)}%  (${current}/${target})`,
    )
  }
}
