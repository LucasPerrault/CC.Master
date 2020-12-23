import { Injectable } from '@angular/core';
import { BehaviorSubject, interval } from 'rxjs';
import { take } from 'rxjs/operators';

import { IToast } from './toasts.model';

@Injectable()
export class ToastsService {
    public toast$ = new BehaviorSubject<Array<IToast>>([]);

    constructor() { }

    public addToast(toast: IToast): string {
        const id = this.generateId();
        this.toast$.next([...this.toast$.value, { ...toast, id }]);

        if (toast.duration) {
            interval(toast.duration)
                .pipe(take(1))
                .subscribe(() => this.removeToast(id));
        }

        return id;
    }

    public removeToast(id: string): void {
        const updatedToastArray = this.toast$.value.filter(toast => toast.id !== id);
        this.toast$.next(updatedToastArray);
    }

    private generateId(): string {
        const randomString = Math.random().toString(36).substr(2, 9);
        return `_${randomString}`;
    }
}
