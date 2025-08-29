import axios from 'axios';

export const getTethysStatus = async () => {
    const response = await axios.get('/api/workflow/tethys-status');
    return response.data;
};

export const updateTethysStatus = async () => {
    const response = await axios.post('/api/workflow/update-tethys-status');
    return response.data;
};
