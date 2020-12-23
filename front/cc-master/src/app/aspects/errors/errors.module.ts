import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ErrorHandler, ModuleWithProviders, NgModule } from '@angular/core';
import { CcErrorHandler } from '@cc/aspects/errors/error.handler';
import { HttpErrorInterceptor } from '@cc/aspects/errors/http-error.interceptor';
import { ToastsService } from '@cc/common/toasts';

@NgModule({})
export class ErrorsModule {
	public static forRoot(): ModuleWithProviders<ErrorsModule> {
		return {
			ngModule: ErrorsModule,
			providers: [
				ToastsService,
				{
					provide: HTTP_INTERCEPTORS,
					useClass: HttpErrorInterceptor,
					multi: true,
				},
        {
          provide: ErrorHandler,
          useClass: CcErrorHandler,
        },
			],
		};
	}
}
