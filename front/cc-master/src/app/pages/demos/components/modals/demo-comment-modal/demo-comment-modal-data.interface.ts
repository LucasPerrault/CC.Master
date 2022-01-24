import { IDemo } from '../../../models/demo.interface';

export enum DemoCommentModalMode {
  Readonly = 'readonly',
  Edition = 'edition',
}

export interface IDemoCommentEditionModalData {
  demo: IDemo;
  mode: DemoCommentModalMode;
}
