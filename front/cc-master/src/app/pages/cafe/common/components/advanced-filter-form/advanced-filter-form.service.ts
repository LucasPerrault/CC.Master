import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';

@Injectable()
export class AdvancedFilterFormService {
  public get reset$(): Observable<void> { return this.resetForm.asObservable(); }
  private resetForm = new ReplaySubject<void>(1);

  public reset(): void {
    this.resetForm.next();
  }

}
