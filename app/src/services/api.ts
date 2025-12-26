import axios from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    withCredentials: true,
});

api.interceptors.response.use(
    (res) => res,
    (err) => {
        const status = err.response?.status;
        if (status === 401) {
            document.cookie = "agenda_token=; Path=/; Max-Age=0";
            window.location.href = "/login";
        }

        return Promise.reject(err);
    }
);

export default api;
