import { useEffect, useState, type JSX } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { Login } from "./pages/Login";
import { Dashboard } from "./pages/Dashborad";
import { checkSession } from "./services/authService";

function PrivateRoute({ children }: { children: JSX.Element }) {
  const [auth, setAuth] = useState<null | boolean>(null);

  useEffect(() => {
    async function validate() {
      const ok = await checkSession();
      setAuth(ok);
    }

    validate();
  }, []);

  if (auth === null) {
    return <p>🔐 Verificando sessão...</p>;
  }

  return auth ? children : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />

        <Route
          path="/dashboard"
          element={
            <PrivateRoute>
              <Dashboard />
            </PrivateRoute>
          }
        />

        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
