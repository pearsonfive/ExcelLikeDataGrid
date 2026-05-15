public static XlCall.XlReturn AtomXlCoerce(out object result, object input, XlType xlType)
{
    // Atom implementation of the coercion in Excel. Allows Atom to avoid calling Excel
    // in background threads.

    // Replacement for:
    // public static XlReturn TryExcel(XlCall.xlCoerce, out object result, params object[] parameters)

    switch (xlType)
    {
        case XlType.XlTypeArray:
            throw new NotImplementedException("XlType.XlTypeArray");

        case XlType.XlTypeBoolean:
            if (input is bool)
            {
                result = input;
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else if (input is double || input is long)
            {
                result = Convert.ToBoolean(input); // Zero is False, all other values are True
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else
            {
                result = null;
                return XlCall.XlReturn.XlReturnFailed;
            }

        case XlType.XlTypeEmpty:
            throw new NotImplementedException("XlType.XlTypeEmpty");

        case XlType.XlTypeError:
            throw new NotImplementedException("XlType.XlTypeError");

        case XlType.XlTypeInt:
            throw new NotImplementedException("XlType.XlTypeInt");

        case XlType.XlTypeMissing:
            throw new NotImplementedException("XlType.XlTypeMissing");

        case XlType.XlTypeNumber:
            if (input is double)
            {
                result = input;
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else if (input is long || input is bool)
            {
                result = Convert.ToDouble(input);
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else if (input is string)
            {
                double resultDouble;
                if (double.TryParse((string)input, out resultDouble))
                {
                    result = resultDouble;
                    return XlCall.XlReturn.XlReturnSuccess;
                }
                else
                {
                    result = null;
                    return XlCall.XlReturn.XlReturnFailed;
                }
            }
            else
            {
                result = null;
                return XlCall.XlReturn.XlReturnFailed;
            }

        case XlType.XlTypeReference:
            throw new NotImplementedException("XlType.XlTypeReference");

        case XlType.XlTypeString:
            if (input is String)
            {
                result = input;
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else if (input is bool)
            {
                result = input.ToString().ToUpper();
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else if (input is double || input is long) // Also covers conversions from dates
            {
                result = input.ToString();
                return XlCall.XlReturn.XlReturnSuccess;
            }
            else
            {
                result = null;
                return XlCall.XlReturn.XlReturnFailed;
            }

        default:
            throw new NotImplementedException("default");
    }
}

 public enum XlType : int
    {
        // See https://github.com/Excel-DNA/ExcelDna/blob/master/Distribution/Samples/Conversions.dna

        XlTypeNumber    = 0x0001,
        XlTypeString    = 0x0002,
        XlTypeBoolean   = 0x0004,
        XlTypeReference = 0x0008,
        XlTypeError     = 0x0010,
        XlTypeArray     = 0x0040,
        XlTypeMissing   = 0x0080,
        XlTypeEmpty     = 0x0100,
        XlTypeInt       = 0x0800,    // int16 in XlOper, int32 in XlOper12, never passed into UDF
    }