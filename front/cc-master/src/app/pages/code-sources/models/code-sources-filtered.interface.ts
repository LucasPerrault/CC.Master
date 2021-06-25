import { ICodeSource } from './code-source.interface';

export interface ICodeSourcesFiltered {
  activeCodeSources: ICodeSource[];
  deletedCodeSources: ICodeSource[];
  referencedCodeSources: ICodeSource[];
}
