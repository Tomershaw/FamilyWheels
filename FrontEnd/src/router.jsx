import Login from "./users/Login";
import Calendar from "./components/Calendar";
import Register from "./users/Register";
import UserName from "./users/UserName";
import { createBrowserRouter } from "react-router-dom";

export const router = createBrowserRouter([
  { path: "/", element: <Login /> },
  { path: "/Calendar", element: <Calendar /> },
  { path: "/Register", element: <Register /> },
  { path: "/UserName", element: <UserName /> },
]);
