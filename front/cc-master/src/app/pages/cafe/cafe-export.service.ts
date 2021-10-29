import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class CafeExportService {
    public get exports(): Observable<void> {
        return this.exports$.asObservable();
    }

    private exports$: Subject<void> = new Subject<void>();

    constructor() {
    }

    public export(): void {
        this.exports$.next();
    }
}
