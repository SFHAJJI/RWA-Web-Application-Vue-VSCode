import api from '../api';
import type { HecateInventaireNormaliseDto, HecateInterneHistoriqueDto } from '../types/bddManager';

export const getOblValidationColumns = () => api.get('/api/workflow/obl-validation-columns');
export const getOblValidationData = () => api.get('/api/workflow/obl-validation-data');
export const submitOblValidation = (data: HecateInventaireNormaliseDto[]) => api.post('/api/workflow/submit-obl-validation', data);

export const getAddToBddColumns = () => api.get('/api/workflow/add-to-bdd-columns');
export const getAddToBddData = () => api.get('/api/workflow/add-to-bdd-data');
export const submitAddToBdd = (data: HecateInterneHistoriqueDto[]) => api.post('/api/workflow/submit-add-to-bdd', data);
