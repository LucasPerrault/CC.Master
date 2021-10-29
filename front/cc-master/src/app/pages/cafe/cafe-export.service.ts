import { Injectable } from '@angular/core';
import { SubmissionState } from '@cc/common/forms';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable()
export class CafeExportService {
    public get exportRequests$(): Observable<void> {
        return this.exportRequestsSubject$.asObservable();
    }

    public get exportState$(): Observable<SubmissionState> {
        return this.exportStateSubject$.asObservable();
    }

    private exportRequestsSubject$: Subject<void> = new Subject<void>();
    private exportStateSubject$: BehaviorSubject<SubmissionState> = new BehaviorSubject<SubmissionState>(SubmissionState.Idle);

    constructor() {
    }

    public requestExport(): void {
        this.exportRequestsSubject$.next();
    }

    public notifyExport(submissionState: SubmissionState): void {
        this.exportStateSubject$.next(submissionState);
    }
}
