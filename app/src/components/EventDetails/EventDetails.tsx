import { useEffect, useState } from "react";
import { getEventById } from "../../services/eventService";
import "./EventDetails.css";

type EventDetailsProps = {
    eventId: string;
    onClose: () => void;
};

export function EventDetails({ eventId, onClose }: EventDetailsProps) {
    const [event, setEvent] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function loadDetails() {
            try {
                const data = await getEventById(eventId);
                setEvent(data);
            } catch (err) {
                alert("Error loading event details");
                console.error(err);
            } finally {
                setLoading(false);
            }
        }
        loadDetails();
    }, [eventId]);

    const formatDate = (date: string) =>
        new Date(date).toLocaleString("pt-BR", {
            dateStyle: "short",
            timeStyle: "short",
        });

    if (loading) {
        return (
            <div className="event-modal-overlay">
                <div className="event-modal">
                    <p>⏳ Loading...</p>
                </div>
            </div>
        );
    }

    if (!event) return null;

    return (
        <div className="event-modal-overlay" onClick={onClose}>
            <div className="event-modal" onClick={(e) => e.stopPropagation()}>

                <header className="event-modal-header">
                    <h2>{event.summary}</h2>
                    <button className="close-btn" onClick={onClose}>✖</button>
                </header>

                <div className="event-modal-content">

                    <p><strong>🕒 Start:</strong> {formatDate(event.start)}</p>
                    <p><strong>🏁 End:</strong> {formatDate(event.end)}</p>

                    {event.location && (
                        <p><strong>📍 Location:</strong> {event.location}</p>
                    )}

                    {event.description && (
                        <p><strong>📝 Description:</strong> {event.description}</p>
                    )}

                    {event.htmlLink && (
                        <a
                            className="google-link"
                            href={event.htmlLink}
                            target="_blank"
                        >
                            Open in Google Calendar ↗
                        </a>
                    )}

                </div>
            </div>
        </div>
    );
}
