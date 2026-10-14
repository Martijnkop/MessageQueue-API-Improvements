const express = require("express");
const { Kafka } = require("kafkajs");

const kafka = new Kafka({
  clientId: "demo-server",
  brokers: process.env.KAFKA_BROKERS.split(","),
});
const producer = kafka.producer();
const topic = process.env.KAFKA_TOPIC;

const app = express();
app.use(express.json());

app.post("/test", async (req, res) => {
  const { message } = req.body;
  if (!message) {
    return res.status(400).json({ error: "Body must include a message" });
  }

  try {
    await producer.send({
      topic,
      messages: [{ value: JSON.stringify({ message, sentAt: new Date().toISOString() }) }],
    });
    res.status(202).json({ status: "queued", message });
  } catch (err) {
    console.error("Failed to send to Kafka:", err);
    res.status(500).json({ error: "Failed to queue message" });
  }
});

async function main() {
  await producer.connect();
  const port = process.env.PORT || 3000;
  app.listen(port, () => console.log(`Server listening on :${port}`));
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});