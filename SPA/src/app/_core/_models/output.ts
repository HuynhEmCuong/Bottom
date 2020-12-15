import { OutputM } from './outputM';
import { MaterialSheetSize } from './material-sheet-size';

export class Output {
    outputs: OutputM[];
    outputTotalNeedQty: OutputTotalNeedQty[];
    message: string;
}

export interface OutputTotalNeedQty {
    order_Size: number;
    model_Size: number;
    tool_Size: number;
    qty: number;
}
