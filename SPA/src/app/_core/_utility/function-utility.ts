import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class FunctionUtility {
    baseUrl = environment.apiUrl;
    /**
     *Hàm tiện ích
     */


    constructor(private http: HttpClient) {
    }

    /**
     *Trả ra ngày hiện tại, chỉ lấy năm tháng ngày: yyyy/MM/dd
     */
    getToDay() {
        const toDay = new Date().getFullYear().toString() +
            '/' + (new Date().getMonth() + 1).toString() +
            '/' + new Date().getDate().toString();
        return toDay;
    }

    /**
     *Trả ra ngày với tham số truyền vào là ngày muốn format, chỉ lấy năm tháng ngày: yyyy/MM/dd
     */
    getDateFormat(day: Date) {
        const dateFormat = day.getFullYear().toString() +
            '/' + (day.getMonth() + 1).toString() +
            '/' + day.getDate().toString();
        return dateFormat;
    }

    /**
     *Trả ra transferNo mới theo yêu cầu: TB(ngày thực hiện yyyymmdd) 3 mã số random number. (VD: TB20200310001)
     */
    async getTransferNo() {
        let transferNo;
        do {
            transferNo =
                'TB' +
                new Date().getFullYear().toString() +
                ((new Date().getMonth() + 1 > 9) ? (new Date().getMonth() + 1).toString() : ('0' + (new Date().getMonth() + 1).toString())) +
                (new Date().getDate() > 9 ? new Date().getDate().toString() : '0' + new Date().getDate().toString()) +
                Math.floor(Math.random() * (999 - 100 + 1) + 100);

        } while (await this.checkTransacNoDuplicate(transferNo));
        return transferNo;
    }

    /**
     *Trả ra outputSheetNo mới theo yêu cầu: BO+(Plan No)+ 3 số random (001,002,003…). VD: BO0124696503001
     */
    async getOutSheetNo(planNo: string) {
        let outputSheetNo;
        do {
            outputSheetNo =
            'BO' + planNo +
            Math.floor(Math.random() * (999 - 100 + 1) + 100);

        } while (await this.checkTransacNoDuplicate(outputSheetNo));
        return outputSheetNo;
    }

    /**
     *Trả ra inputNo mới theo yêu cầu: BI+(Plan No)+ 3 số random (001,002,003…). VD: BI0124696503001
     */
    async getInputNo(planNo: string) {
        let inputNo;
        do {
            inputNo =
            'BI' + planNo +
            Math.floor(Math.random() * (999 - 100 + 1) + 100);

        } while (await this.checkTransacNoDuplicate(inputNo));
        return inputNo;
    }

    async checkTransacNoDuplicate(transacNo: string) {
        const result = await this.http.get<boolean>(this.baseUrl + 'TransferLocation/CheckTransacNoDuplicate',
            { params: { transacNo: transacNo } }).toPromise();
        return result;
    }
    
    /**
     * Nhập vào kiểu chuỗi hoặc số dạng 123456789 sẽ trả về 123,456,789
     */
    convertNumber(amount) {
        return String(amount).replace(/(?<!\..*)(\d)(?=(?:\d{3})+(?:\.|$))/g, '$1,')
    }
    
    /**
     * Check 1 string có phải empty hoặc null hoặc undefined ko.
     */
    checkEmpty(str: string) {
        return (!str || /^\s*$/.test(str));
    }

    checkQrCodeOfLength(str: string) {
        if(str.length < 14) {
            return false;
        } else if(str.length === 14) {
            let arrayStr = str.split('');
            if(arrayStr[0] === 'B') {
                return true;
            } else {
                return false;
            }
        } else if(str.length === 15) {
            return true;
        }
    }
}
