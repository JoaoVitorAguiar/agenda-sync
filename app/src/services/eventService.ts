import api from "./api";

export async function getEvents() {
    const { data } = await api.get("/events", { withCredentials: true });
    return data;
}
