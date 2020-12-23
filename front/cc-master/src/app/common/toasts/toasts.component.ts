import { ChangeDetectionStrategy,Component } from '@angular/core';

import { IToast, ToastType } from './toasts.model';
import { ToastsService } from './toasts.service';

@Component({
    selector: 'cc-toasts',
    templateUrl: './toasts.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastsComponent {
    public toasts$ = this.toastsService.toast$;

    constructor(private toastsService: ToastsService) { }

    public getIconId(toast: IToast): string {
        switch (toast.type) {
            case ToastType.Success:
                return 'success';
            case ToastType.Error:
                return 'error';
            case ToastType.Warning:
                return 'warning';
            default:
                return '';
        }
    }

    public remove(id: string): void {
        this.toastsService.removeToast(id);
    }
}
