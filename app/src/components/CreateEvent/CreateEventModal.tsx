import { useState } from "react";
import { createEvent } from "../../services/eventService";
import "./CreateEventModal.css";

interface Props {
    onClose: () => void;
    onCreated?: () => void;
}

export function CreateEventModal({ onClose, onCreated }: Props) {
    const [summary, setSummary] = useState("");
    const [start, setStart] = useState("");
    const [end, setEnd] = useState("");
    const [location, setLocation] = useState("");
    const [description, setDescription] = useState("");
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setLoading(true);

        try {
            await createEvent({
                summary,
                start: start + ":00",
                end: end + ":00",
                description,
                location,
                timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
            });

            alert("Event created successfully!");
            onCreated?.();
            onClose();

        } catch (err) {
            console.error(err);
            alert("Error creating event.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="modal-overlay" onClick={onClose}>
            <form className="modal" onClick={(e) => e.stopPropagation()} onSubmit={handleSubmit}>

                <header className="modal-header">
                    <h2>Create Event</h2>
                    <button type="button" className="close-btn" onClick={onClose}>✖</button>
                </header>

                <label>
                    Title*
                    <input
                        required
                        value={summary}
                        onChange={(e) => setSummary(e.target.value)}
                        placeholder="e.g. Team Meeting"
                    />
                </label>

                <label>
                    Start*
                    <input
                        type="datetime-local"
                        required
                        value={start}
                        onChange={(e) => setStart(e.target.value)}
                    />
                </label>

                <label>
                    End*
                    <input
                        type="datetime-local"
                        required
                        value={end}
                        onChange={(e) => setEnd(e.target.value)}
                    />
                </label>

                <label>
                    Location
                    <input
                        value={location}
                        onChange={(e) => setLocation(e.target.value)}
                        placeholder="Google Meet / Office / etc."
                    />
                </label>

                <label>
                    Description
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        placeholder="Notes, objectives, participants..."
                    />
                </label>

                <button className="btn-primary" disabled={loading}>
                    {loading ? "Creating..." : "Create Event"}
                </button>

            </form>
        </div>
    );
}
