import axios from 'axios';

// This will work in a production environment because the API calls will be relative to the current domain.
const apiClient = axios.create({
  baseURL: '/',
  headers: {
    'Content-Type': 'application/json',
  },
});

import { AxiosRequestConfig } from 'axios';

export const post = (url: string, data: any, config?: AxiosRequestConfig) => {
  return apiClient.post(url, data, {
    ...config,
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
      ...config?.headers,
    }
  });
};

export default apiClient;
