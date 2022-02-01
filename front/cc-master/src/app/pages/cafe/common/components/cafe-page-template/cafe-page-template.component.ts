import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

import { ICategory } from '../../forms/select/category-select/category.interface';

@Component({
  selector: 'cc-cafe-page-template',
  templateUrl: './cafe-page-template.component.html',
})
export class CafePageTemplateComponent {
  public categories: ICategory[] = [];
  public category: FormControl = new FormControl();
}
