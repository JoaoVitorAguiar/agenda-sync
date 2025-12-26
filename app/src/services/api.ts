import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5069",
    withCredentials: true,
});

api.interceptors.response.use(
    (res) => res,
    (err) => {
        const status = err.response?.status;
        const title = err.response?.data?.title;

        if (status === 401 && title === "Refresh Token Expired") {
            document.cookie = "agenda_token=; Max-Age=0";
            window.location.href = "/login";
        }

        return Promise.reject(err);
    }
);

export default api;
