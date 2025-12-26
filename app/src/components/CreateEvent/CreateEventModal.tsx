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
                start,
                end,
                description,
                location,
                timeZone: "America/Fortaleza",
            });

            alert("Evento criado com sucesso!");
            onCreated?.();
            onClose();

        } catch (err) {
            console.error(err);
            alert("Erro ao criar evento.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="modal-overlay" onClick={onClose}>
            <form className="modal" onClick={(e) => e.stopPropagation()} onSubmit={handleSubmit}>

                <header className="modal-header">
                    <h2>Criar Evento</h2>
                    <button type="button" className="close-btn" onClick={onClose}>✖</button>
                </header>

                <label>
                    Título*
                    <input
                        required
                        value={summary}
                        onChange={(e) => setSummary(e.target.value)}
                        placeholder="Ex: Reunião Equipe"
                    />
                </label>

                <label>
                    Início*
                    <input
                        type="datetime-local"
                        required
                        value={start}
                        onChange={(e) => setStart(e.target.value)}
                    />
                </label>

                <label>
                    Fim*
                    <input
                        type="datetime-local"
                        required
                        value={end}
                        onChange={(e) => setEnd(e.target.value)}
                    />
                </label>

                <label>
                    Local
                    <input
                        value={location}
                        onChange={(e) => setLocation(e.target.value)}
                        placeholder="Google Meet / Escritório / etc."
                    />
                </label>

                <label>
                    Descrição
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        placeholder="Notas, objetivos, participantes..."
                    />
                </label>

                <button className="btn-primary" disabled={loading}>
                    {loading ? "Criando..." : "Criar Evento"}
                </button>

            </form>
        </div>
    );
}
