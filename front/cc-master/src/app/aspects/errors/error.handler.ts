import { ErrorHandler, Injectable } from '@angular/core';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { LuSentryErrorHandler } from '@lucca/sentry/ng';


@Injectable()
export class CcErrorHandler implements ErrorHandler {
  constructor(private toastsService: ToastsService, private luSentryErrorHandler: LuSentryErrorHandler) {}

  handleError(error: Error) {
    this.toastsService.addToast({
      message: `${error.message}`,
      type: ToastType.Error,
    });

    this.luSentryErrorHandler.handleError(error);
  }
}
