export class Qot {
  //h?: RfqH;
  m?: matnr[];
  q?: QotV;
  reject?:reject;
}

export class QotH {
  qotId?: number;
  qotNum?: string;
  status?: number;
  vendorId?: string;
  matnrId?: string;
  rfqId?: string;
  leadTime?: string;
  minQty?: number;
  totalAmount?: number;
  createDate?: Date;
  createBy?: string;
  lastUpdateDate?: Date;
  lastUpdateBy?: string;
  }

  export class QotV {
    qotId?: number;
    qotNum?: string;
    rfqNum?: string;
    size?: string;
    matnrId?: number;
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
    status ?: number;
    createDate?: Date;
    createBy ?: string;
    //lastUpdateDate ?: Date;
    //lastUpdateBy ?: string;
  }
  export class matnr {
    qotId?: number;
    rfqId?: number;
    vendorId?: number;
    matnr?: string;
    rfqNum?: string;
    matnrId?: number;
    deadline?:Date;
    werks?:string;
  }

  export class reject {
    reason?: string;
  }

