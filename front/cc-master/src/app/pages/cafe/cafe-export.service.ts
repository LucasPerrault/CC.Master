import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable()
export class CafeExportService {
    public get exportRequests$(): Observable<void> {
        return this.exportRequestsSubject$.asObservable();
    }

    public get isExporting$(): Observable<boolean> {
        return this.isExporting$Subject$.asObservable();
    }

    private exportRequestsSubject$: Subject<void> = new Subject<void>();
    private isExporting$Subject$: Subject<boolean> = new Subject<boolean>();

    constructor() {
    }

    public requestExport(): void {
        this.exportRequestsSubject$.next();
    }

    public notifyExport(isExporting: boolean): void {
        this.isExporting$Subject$.next(isExporting);
    }
}
