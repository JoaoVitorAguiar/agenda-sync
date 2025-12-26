import { useEffect, useState } from "react";
import { getEvents } from "../services/eventService";
import { logout } from "../services/authService";

import "./Dashboard.css";

interface Event {
    summary: string;
    start: string;
    end: string;
}

export function Dashboard() {
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function loadEvents() {
            try {
                const data = await getEvents(); // 👈 usando o service
                setEvents(data);
            } catch (error) {
                console.error("Erro ao buscar eventos:", error);
                alert("Sessão expirada. Faça login novamente.");
                window.location.href = "/";
            } finally {
                setLoading(false);
            }
        }

        loadEvents();
    }, []);

    if (loading) return <p className="loading">Carregando eventos...</p>;

    return (
        <div className="dashboard-container">

            <header className="dashboard-header">
                <h1>📅 Seus próximos eventos</h1>
                <button className="logout-btn" onClick={logout}>
                    Sair
                </button>
            </header>

            {events.length === 0 && (
                <p className="no-events">Nenhum evento encontrado.</p>
            )}

            <div className="events-grid">
                {events.map((event, index) => (
                    <div className="event-card" key={index}>
                        <h2>{event.summary}</h2>
                        <p className="date">
                            <span>🕒 </span>
                            {new Date(event.start).toLocaleString("pt-BR")}
                        </p>
                        <p className="date">
                            <span>🏁 </span>
                            {new Date(event.end).toLocaleString("pt-BR")}
                        </p>
                    </div>
                ))}
            </div>
        </div>
    );
}
