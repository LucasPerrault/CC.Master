import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { HttpErrorInterceptor } from '@cc/aspects/http-error/http-error.interceptor';

import { ToastsService } from '../../common/toasts';

@NgModule({})
export class HttpErrorModule {
	public static forRoot(): ModuleWithProviders<HttpErrorModule> {
		return {
			ngModule: HttpErrorModule,
			providers: [
				ToastsService,
				{
					provide: HTTP_INTERCEPTORS,
					useClass: HttpErrorInterceptor,
					multi: true,
				},
			],
		};
	}
}
