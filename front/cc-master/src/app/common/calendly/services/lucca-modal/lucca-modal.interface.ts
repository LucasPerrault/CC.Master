import { EventEmitter } from '@angular/core';

export interface ILuccaModal {
  close$: EventEmitter<void>;
}
