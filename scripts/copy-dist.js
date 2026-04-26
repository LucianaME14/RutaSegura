import fs from "fs/promises";
import path from "path";

const root = path.resolve("./");
const source = path.join(root, "dist");
const destination = path.join(root, "backend", "wwwroot");

async function copyDir(src, dest) {
  await fs.mkdir(dest, { recursive: true });
  const entries = await fs.readdir(src, { withFileTypes: true });
  for (const entry of entries) {
    const srcPath = path.join(src, entry.name);
    const destPath = path.join(dest, entry.name);
    if (entry.isDirectory()) {
      await copyDir(srcPath, destPath);
    } else if (entry.isFile()) {
      await fs.copyFile(srcPath, destPath);
    }
  }
}

async function main() {
  try {
    const sourceStat = await fs.stat(source).catch(() => null);
    if (!sourceStat || !sourceStat.isDirectory()) {
      throw new Error(
        "Build frontend first: run 'npm run build' before copying dist files.",
      );
    }

    await fs.rm(destination, { recursive: true, force: true });
    await copyDir(source, destination);
    console.log(`Copied frontend build from ${source} to ${destination}`);
  } catch (error) {
    console.error("Error copying frontend build:", error.message ?? error);
    process.exit(1);
  }
}

main();
