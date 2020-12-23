import { ErrorHandler, Injectable } from '@angular/core';

import { ToastsService, ToastType } from '../../common/toasts';


@Injectable()
export class CcErrorHandler implements ErrorHandler {
  constructor(private toastsService: ToastsService) {}

  handleError(error: Error) {
    this.toastsService.addToast({
      message: `Erreur : ${error.message}`,
      type: ToastType.Error,
    });
  }
}
