import { Component, forwardRef } from '@angular/core';
import { ALuOptionOperator, ILuOptionOperator } from '@lucca-front/ng/option';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'cc-api-select-empty-state',
  templateUrl: './api-select-empty-state.component.html',
  providers: [
    {
      provide: ALuOptionOperator,
      useExisting: forwardRef(() => ApiSelectEmptyStateComponent),
      multi: true,
    },
  ],
})
export class ApiSelectEmptyStateComponent<T = any> extends ALuOptionOperator<T> implements ILuOptionOperator<T> {
  outOptions$: Observable<T[]>;

  public get isEmpty$(): Observable<boolean> {
    return this.outOptions$?.pipe(map(options => !options?.length));
  }

  set inOptions$(in$: Observable<T[]>) {
    this.outOptions$ = in$;
  }
}
