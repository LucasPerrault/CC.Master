import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ToastsService, ToastType } from '../../common/toasts';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

  constructor(private toastsService: ToastsService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((httpErrorResponse: HttpErrorResponse) => {

        const message = httpErrorResponse.error instanceof ErrorEvent
          ? this.getClientSideErrorMessage(httpErrorResponse)
          : this.getServerSideErrorMessage(httpErrorResponse);

        this.toastsService.addToast({
          message,
          type: ToastType.Error,
        });

        return throwError(httpErrorResponse);
      }),
    );
  }

  private getClientSideErrorMessage(httpErrorResponse: HttpErrorResponse): string {
    return `Erreur : ${httpErrorResponse.error.message}`;
  }

  private getServerSideErrorMessage(httpErrorResponse: HttpErrorResponse): string {
    const message = httpErrorResponse.error.Message;
    const status = httpErrorResponse.status;
    const statusText = httpErrorResponse.statusText;
    return `Erreur ${status} ${statusText} : ${message}`;
  }
}
