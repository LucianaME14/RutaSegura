import express from "express";
import { createClient } from "redis";
import "dotenv/config";

const app = express();
const client = createClient({ url: process.env.REDIS_URL });

client.on("error", (err) => console.log("Redis Error", err));
await client.connect();

app.get("/datos", async (req, res) => {
  const info = await client.get("mi_clave");
  res.json({ data: info });
});

app.listen(3000, () => console.log("Servidor listo"));
