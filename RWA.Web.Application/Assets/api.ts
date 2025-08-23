import axios from 'axios';

// This will work in a production environment because the API calls will be relative to the current domain.
const apiClient = axios.create({
  baseURL: '/',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const post = (url: string, data: any) => {
  return apiClient.post(url, data, {
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    }
  });
};

export default apiClient;
