import { RouterProvider } from "react-router";
import { router } from "./routes";
import { SafeBot } from "./components/SafeBot";

export default function App() {
  return (
    <>
      <RouterProvider router={router} />
      <SafeBot />
    </>
  );
}