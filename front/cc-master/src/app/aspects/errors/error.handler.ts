import { ErrorHandler, Injectable } from '@angular/core';
import { ToastsService, ToastType } from '@cc/common/toasts';


@Injectable()
export class CcErrorHandler implements ErrorHandler {
  constructor(private toastsService: ToastsService) {}

  handleError(error: Error) {
    this.toastsService.addToast({
      message: `${error.message}`,
      type: ToastType.Error,
    });
  }
}
