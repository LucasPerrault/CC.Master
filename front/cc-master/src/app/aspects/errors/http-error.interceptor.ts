import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';


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
    return `${httpErrorResponse.error.message}`;
  }

  private getServerSideErrorMessage(httpErrorResponse: HttpErrorResponse): string {
    const message = this.getHttpErrorMessage(httpErrorResponse);
    const status = httpErrorResponse.status;
    const statusText = httpErrorResponse.statusText;
    return `${status} ${statusText} ${message}`;
  }

  private getHttpErrorMessage(httpErrorResponse: HttpErrorResponse): string {
    const message = httpErrorResponse.error?.Message
      || httpErrorResponse.error?.message
      || httpErrorResponse.error?.Detail
      || httpErrorResponse.error?.detail;

    return !!message ? `: ${ message }` : '';
  }
}
