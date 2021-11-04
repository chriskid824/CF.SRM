import { getLocaleDateTimeFormat } from "@angular/common";
import { ThisReceiver } from "@angular/compiler";

export class AnnouncementType {
    stylecolor: string;
    txtTypeName: string;
    main_Announcement: Main_Announcement[];
    numberList:string[];
    icon: string;
    router:string;

    constructor(stylecolor: string, txtTypeName: string, icon: string, main_Announcement: Main_Announcement[],numberList:string[],router:string) {
        this.stylecolor = stylecolor;
        this.txtTypeName = txtTypeName;
        this.icon = icon;
        this.main_Announcement = main_Announcement;
        this.numberList=numberList;
        this.router=router;
    }
    public getSearch(): string {
        switch (this.txtTypeName) {
            case "採購單":
                return "po";
            default:
                return "#";
        }
    }Link
    public getLink(number): string {
      return this.router+'/'+number;
    }
}

export class Main_Announcement {
    CASEID: string;
    txtSN: string;
    txtSubject: string; //主旨
    txtTypeName: string;
    StartDate: string;
    txtUser: string;
    txtExt: string;
    richText001: string;
    uploadFile001: string;
    constructor(CASEID: string, txtSN: string, txtSubject: string, txtTypeName: string, StartDate: string, txtUser: string, txtExt: string, richText001: string, uploadFile001: string) {
        this.CASEID = CASEID;
        this.txtSN = txtSN;
        this.txtSubject = txtSubject;
        this.txtTypeName = txtTypeName;
        this.StartDate = StartDate;
        this.txtUser = txtUser;
        this.txtExt = txtExt;
        this.richText001 = richText001;
        this.uploadFile001 = uploadFile001;
    }
    Main_Announcement(data: any, txtTypeName: string):Main_Announcement[] {
        let result: Main_Announcement[]=[];
        if (data != null) {
            let filterdata = data.filter(x => x.txtTypeName === txtTypeName).sort(p=>p.StartDate).slice(0,5);
            if (filterdata != null && filterdata.length>0) {
                filterdata.forEach(element => {
                    result.push(new Main_Announcement(element.CASEID,
                        element.txtSN,element.txtSubject,element.txtTypeName
                        ,element.StartDate,element.txtUser,element.txtExt,element.richText001,element.uploadFile001));
                });
                return result;
            }
        }
        return [new Main_Announcement('','A00000000','暫無公告',txtTypeName,new Date().getDate().toString(),'簡建豪','3333','','')];
    }
}
