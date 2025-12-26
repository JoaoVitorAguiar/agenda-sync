import api from "./api";

export async function getEvents() {
    const { data } = await api.get("/events", { withCredentials: true });
    return data;
}

export async function getEventById(id: string) {
    const { data } = await api.get(`/events/${id}`);
    return data;
}

export async function createEvent(event: any) {
    const { data } = await api.post("/events", event);
    return data;
}
