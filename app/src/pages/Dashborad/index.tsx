import { useEffect, useState } from "react";
import { getEvents } from "../../services/eventService";
import { AppLayout } from "../../components/Layout/AppLayout";
import { EventDetails } from "../../components/EventDetails/EventDetails";
import { CreateEventModal } from "../../components/CreateEvent/CreateEventModal";
import "./styles.css";

interface Event {
    id: string;
    summary: string;
    start: string;
    end: string;
}

export function Dashboard() {
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedId, setSelectedId] = useState<string | null>(null);
    const [openCreate, setOpenCreate] = useState(false);

    async function loadEvents() {
        try {
            setLoading(true);
            const data = await getEvents();
            setEvents(data);
        } catch (error) {
            console.error("Erro ao buscar eventos:", error);
            alert("Sessão expirada. Faça login novamente.");
            window.location.href = "/login";
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadEvents();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    if (loading) return <p className="loading">Carregando eventos...</p>;

    return (
        <>
            <AppLayout>
                <div className="dashboard-container">
                    <div className="dashboard-topbar">
                        <h1>
                            <span className="emoji">📅</span>
                            <span className="title-text">Seus próximos eventos</span>
                        </h1>

                        <button className="btn-create" onClick={() => setOpenCreate(true)}>
                            ➕ Novo Evento
                        </button>
                    </div>

                    {events.length === 0 && <p className="no-events">Nenhum evento encontrado.</p>}

                    <div className="events-grid">
                        {events.map((event) => (
                            <div
                                className="event-card"
                                key={event.id}
                                onClick={() => setSelectedId(event.id)}
                                role="button"
                                tabIndex={0}
                                onKeyDown={(e) => e.key === "Enter" && setSelectedId(event.id)}
                                style={{ cursor: "pointer" }}
                            >
                                <h2>{event.summary ?? "(Sem título)"}</h2>
                                <p className="date">🕒 {new Date(event.start).toLocaleString("pt-BR")}</p>
                                <p className="date">🏁 {new Date(event.end).toLocaleString("pt-BR")}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </AppLayout>

            {selectedId && <EventDetails eventId={selectedId} onClose={() => setSelectedId(null)} />}

            {openCreate && (
                <CreateEventModal
                    onClose={() => setOpenCreate(false)}
                    onCreated={() => {
                        setOpenCreate(false);
                        loadEvents();
                    }}
                />
            )}
        </>
    );
}
