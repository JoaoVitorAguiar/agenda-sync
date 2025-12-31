import { logout } from "../../services/authService";
import "./AppLayout.css";

export function AppLayout({ children }: { children: React.ReactNode }) {
    return (
        <div className="layout-container">
            <header className="layout-header">
                <span className="brand">📅 AgendaSync</span>
                <button className="btn btn-outline logout-btn" onClick={logout}>
                    Logout
                </button>
            </header>

            <main>{children}</main>

            <footer>Made by Vitor Aguiar • 2025</footer>
        </div>
    );
}
