import { InjectionToken } from '@angular/core';
import { IPrincipal } from './principal.interface';

export const PRINCIPAL = new InjectionToken<IPrincipal>('Principal');
