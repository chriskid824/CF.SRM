export class FileType {
  typeId: number;
  typeName?: string;
}
export class Function {
  functionId: number;
  functionName?: string;
}
export class BaseSelect {
  id: number;
  name?: string;
}
export class ViewSrmFileUploadTemplate {
  templateId: number;
  templateType: number;
  werks: number;
  type: number;
  effectiveDate: Date;
  deadline: Date;
  filetype : string;
 filetype1 : string;
 filetype2 : string;
 filetype3 : string;
 filetype4 : string;
 filetype5 : string;
 filetype6 : string;
 filetype7 : string;
 filetype8 : string;
 filetype9 : string;
 filetype10: string;
 filetype11: string;
 filetype12: string;
 filetype13: string;
 filetype14: string;
 filetype15: string;
 filetype16: string;
 filetype17: string;
 filetype18: string;
 filetype19: string;
 filetype20: string;
}
export class ViewSrmFileUploadRecordH
{
  recordHId?:number;
  templateId?:number;
  number?:string;
  createDate?:Date;
  createBy?:string;
  lastUpdateDate?:Date;
  lastUpdateBy?:string;
  srmFileuploadRecordL:ViewSrmFileUploadRecordL[] ;
}
export class ViewSrmFileUploadRecordL
{
  recordLId?:number;
  recordHId?:number;
  filetype?:number;
  url?:string;
  enable?:string;
  createDate?:Date;
  createBy?:string;
  lastUpdateDate?:Date;
  lastUpdateBy?:string;
  filename?:string;
  filetypename?:string;
}

export class ViewSrmFileRecord
{
  recordLId?:number;
  recordHId?:number;
  templateId?:number;
  number?:string;
  templateType?:number;
  functionName?:string;
  werks?:number;
  type?:number;
  effectiveDate?:Date;
  deadline?:Date;
  filetype?:string;
  typeName?:string;
  url?:string;
  createDate?:Date;
  createBy?:string;
  lastUpdateDate?:Date;
  lastUpdateBy?:string;
  fileList:any[];
}
export class FileEmit
{
  filtType:string;
  file:File[];
}
