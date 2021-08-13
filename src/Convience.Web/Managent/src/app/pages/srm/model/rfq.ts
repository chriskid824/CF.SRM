export class Rfq {
  h?: RfqH;
  m?: RfqM[];
  v?: RfqV[];
}

export class RfqH {
  rfqId?: number;
  rfqNum?: string;
  status?: number;
  sourcer?: string;
  sourcerName?: string;
  deadline?: Date;
  createDate?: Date;
  createBy?: string;
  lastUpdateDate?: Date;
  lastUpdateBy?: string;
  endDate?: Date;
  endBy?: string;
  viewstatus?: string;
  c_date?: string;
  c_by?: string;
  deadline_str?: string;
}
export class RfqM {
  rfqMId?: number;
  rfqId?: number;
  matnrId?: number;
  version?: string;
  material?: string;
  length?:number;
  width?: number;
  height?: number;
  density?: string;
  weight?: number;
  machineName?: string;
  qty?: number;
  note?: string;
  matnr?: string;
  srmMatnr1?: string;
}

export class RfqV {
  rfqVId?: number;
  rfqId?: number;
  vendorId?: number;
  srmVendor1?: string;
  vendor?: string;
  vendorName?: string;
  org?: number;
  ekorg ?: number;
  person ?: string;
  address ?: string;
  telPhone ?: string;
  ext ?: string;
  faxNumber ?: string;
  cellPhone ?: string;
  mail ?: string;
  status ?: number;
  createDate?: Date;
  createBy ?: string;
  lastUpdateDate ?: Date;
  lastUpdateBy ?: string;
}

