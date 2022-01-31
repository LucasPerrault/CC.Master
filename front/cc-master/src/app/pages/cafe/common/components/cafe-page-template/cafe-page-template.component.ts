import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

import { ICategory } from '../../cafe-filters/category-filter/category-select/category.interface';

@Component({
  selector: 'cc-cafe-page-template',
  templateUrl: './cafe-page-template.component.html',
})
export class CafePageTemplateComponent {
  public categories: ICategory[] = [];
  public category: FormControl = new FormControl();
}
