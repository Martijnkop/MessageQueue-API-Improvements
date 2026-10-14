const { Kafka } = require("kafkajs");

const kafka = new Kafka({
  clientId: "demo-worker",
  brokers: process.env.KAFKA_BROKERS.split(","),
});
const consumer = kafka.consumer({ groupId: "demo-workers" });

async function main() {
  await consumer.connect();
  await consumer.subscribe({ topic: process.env.KAFKA_TOPIC, fromBeginning: true });

  await consumer.run({
    eachMessage: async ({ topic, partition, message }) => {
      const payload = JSON.parse(message.value.toString());
      console.log(payload);
    },
  });

  console.log("Worker waiting for messages...");
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});