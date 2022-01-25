import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class DemoDuplicationsService {

  public get duplicationIds$(): Observable<string[]> {
    return this.duplicationIds.asObservable();
  }

  public duplicationIds = new BehaviorSubject<string[]>([]);

  public add(duplicationId: string): void {
    this.duplicationIds.next([...this.duplicationIds.value, duplicationId]);
  }

  public remove(duplicationId: string): void {
    this.duplicationIds.next(this.duplicationIds.value.filter(d => d !== duplicationId));
  }
}


