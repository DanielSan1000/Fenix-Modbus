## TableView

The dedicated editor has been crafted to present tags in a tabular format, providing users with a comprehensive interface for managing and interacting with tags in their project. This powerful tool not only enhances the visualization of tags but also facilitates real-time adjustments and observations.

### Key Features:

1. **Table Display:**
   - The editor organizes tags in a structured table format for easy accessibility and clear presentation.

2. **Parameter Modification:**
   - Users can conveniently change the main parameters of each tag directly within the editor. This includes modifying properties such as color, tag name, data type, and more.

3. **Observation of Values:**
   - Real-time observation of tag values is a crucial aspect of the editor. Users can monitor the current state of tags, aiding in troubleshooting and data analysis.

4. **Value Configuration:**
   - Beyond observation, the editor allows users to actively set new values for tags. This dynamic capability is essential for tasks such as remote device configuration or quick adjustments in industrial settings.

5. **Colorization for Identification:**
   - The inclusion of colorization enhances user experience by providing a visual cue for easy identification of different tags. Colors can be customized and are also reflected in the ChartView.

6. **Global Tag Naming:**
   - Each tag requires a global name, ensuring uniqueness within the project. Fenix enforces this rule to prevent naming conflicts, fostering a well-organized and error-free project structure.

7. **Siemens S7/300/400 Support:**
   - For users employing Siemens S7/300/400 drivers, the editor includes a dedicated parameter (DB) for setting the DataBlock address. This ensures seamless integration and compatibility.

8. **Byte and Bit Addressing:**
   - To cater to the nuances of different devices, the editor supports specifying both start addresses and bit/byte addressing. This flexibility is particularly useful when dealing with diverse industrial protocols.

9. **Data Type Selection:**
   - The editor allows users to choose the data type for each tag. Upon fetching data from the device, the driver converts it into the selected data type, ensuring consistency and compatibility.

10. **Memory Area Specification:**
    - Users can inform the driver about the specific part of memory to fetch, enhancing precision and efficiency in data retrieval.

11. **Bytes Order Configuration:**
    - To accommodate variations in data storage, the bytes order parameter is included, ensuring that fetched data is correctly ordered before conversion.

12. **Progress Monitoring:**
    - A progress bar is included in the editor to visually indicate the status of the driver, providing users with insights into ongoing operations.

13. **Description Field:**
    - Each tag includes a description field where users can provide additional context or information. This aids in documentation and improves overall project understanding.

This comprehensive editor stands as a pivotal component in the Fenix software suite, offering an intuitive and efficient solution for managing and interacting with industrial tags. Whether it's configuring parameters, monitoring values, or actively setting new data, the editor streamlines the workflow for users in industrial automation settings.


Editor was created to display tags in table. Editor allow you to change main parameters form there, observe value and set value.

![Image](<lib/NewItem31.png>)

| Parameter      | Description                                                                                                     |
|----------------|-----------------------------------------------------------------------------------------------------------------|
| **Color**      | Colorizing is used to easily identify tags. Colors are set in ChartView as well.                                |
| **Tag Name**   | Global tag name (should be unique in the project). Fenix will not allow you to use two of the same names in one project. |
| **DB**         | Used only for Siemens S7/300/400 drivers. The parameter is used to set the DataBlock address.                     |
| **Start**      | Address from which the driver will start fetching data for this tag.                                              |
| **Bit / Byte** | Second address for fetching data. For example, if you try to get the third BIT from the second register [16bit], use Start->2 and Bit/Byte->3. |
| **Data Type**  | This parameter allows you to choose a data type. After the driver fetches data from the device, it will convert it into this data type. |
| **Memory Area**| Parameter to inform the driver which part of memory should be fetched.                                             |
| **Bytes Order**| Parameter is used to inform the driver how to order fetched data before converting it into the selected data type.|
| **Value**      | Field is used to show the actual value.                                                                          |
| **Progress**   | Field contains a progress bar indicating the status of the driver.                                                |
| **Set Value**  | Used to set data in the remote device.                                                                           |
| **Description**| Field contains description data provided by the user.                                                            |


## Tags Visibility

Parameter below give you possibility to show or hide tag in TableView

![Image](<lib/NewItem14.png>)

## Tag Value Display Formatting

Fenix allow you to format display value in different ways. You can use formatting string to control display.

![Image](<lib/NewItem13.png>)


### Standard Numeric Format Strings

Standard numeric format strings are used to format common numeric types. A standard numeric format string takes the form Axx, where:

* A is a single alphabetic character called the format specifier. Any numeric format string that contains more than one alphabetic character, including white space, is interpreted as a custom numeric format string.
* xx is an optional integer called the precision specifier. The precision specifier ranges from 0 to 99 and affects the number of digits in the result. Note that the precision specifier controls the number of digits in the string representation of a number. It does not round the number itself.


Example:&nbsp; &nbsp; Format Value - \> {0:X2}

| **"C" or "c" Currency** 123.456 ("C", en-US) -\> $123.46 123.456 ("C", fr-FR) -\> 123,46 € 123.456 ("C", ja-JP) -\> ¥123 -123.456 ("C3", en-US) -\> ($123.456) -123.456 ("C3", fr-FR) -\> -123,456 € -123.456 ("C3", ja-JP) -\> -¥123.456 | **"D" or "d" Decimal** 1234 ("D") -\> 1234 -1234 ("D6") -\> -001234 | **"E" or "e" Exponential (scientific)** 1052.0329112756 ("E", en-US) -\> 1.052033E+003 1052.0329112756 ("e", fr-FR) -\> 1,052033e+003 -1052.0329112756 ("e2", en-US) -\> -1.05e+003 -1052.0329112756 ("E2", fr\_FR) -\> -1,05E+003 | **"F" or "f" Fixed-point** 1234.567 ("F", en-US) -\> 1234.57 1234.567 ("F", de-DE) -\> 1234,57 1234 ("F1", en-US) -\> 1234.0 1234 ("F1", de-DE) -\> 1234,0 -1234.56 ("F4", en-US) -\> -1234.5600 -1234.56 ("F4", de-DE) -\> -1234,5600 |
| --- | --- | --- | --- |
| **"G" or "g" General** -123.456 ("G", en-US) -\> -123.456 -123.456 ("G", sv-SE) -\> -123,456 123.4546 ("G4", en-US) -\> 123.5 123.4546 ("G4", sv-SE) -\> 123,5 -1.234567890e-25 ("G", en-US) -\> -1.23456789E-25 -1.234567890e-25 ("G", sv-SE) -\> -1,23456789E-25 | **"X" or "x" Hexadecimal** 255 ("X") -\> FF -1 ("x") -\> ff 255 ("x4") -\> 00ff -1 ("X4") -\> 00FF | **"R" or "r" Round-trip** 123456789.12345678 ("R") -\> 123456789.12345678 -1234567890.12345678 ("R") -\> -1234567890.1234567 | **"P" or "p" Percent** 1 ("P", en-US) -\> 100.00 % 1 ("P", fr-FR) -\> 100,00 % -0.39678 ("P1", en-US) -\> -39.7 % -0.39678 ("P1", fr-FR) -\> -39,7 % |
| **"N" or "n" Number** 1234.567 ("N", en-US) -\> 1,234.57 1234.567 ("N", ru-RU) -\> 1 234,57 1234 ("N1", en-US) -\> 1,234.0 1234 ("N1", ru-RU) -\> 1 234,0 -1234.56 ("N3", en-US) -\> -1,234.560 -1234.56 ("N3", ru-RU) -\> -1 234,560 | &nbsp; | &nbsp; | &nbsp; |

#### Custom Numeric Format Strings

You can create a custom numeric format string, which consists of one or more custom numeric specifiers, to define how to format numeric data. A custom numeric format string is any format string that is not a standard numeric format string.

Example:&nbsp; &nbsp; Format Value - \> {0:00.00}, {0:(###) ###-####}, {0:\[##-##-##\]}, {"Value: {0}"}

| **"0" Zero placeholder** Replaces the zero with the corresponding digit if one is present; otherwise, zero appears in the result string. 1234.5678 ("00000") -\> 01235 0.45678 ("0.00", en-US) -\> 0.46 0.45678 ("0.00", fr-FR) -\> 0,46 | **"#" Digit placeholder** Replaces the "#" symbol with the corresponding digit if one is present; otherwise, no digit appears in the result string.&nbsp; Note that no digit appears in the result string if the corresponding digit in the input string is a non-significant 0. For example, 0003 ("####") -\> 3. 1234.5678 ("#####") -\> 1235 0.45678 ("#.##", en-US) -\> .46 0.45678 ("#.##", fr-FR) -\> ,46 | **"." Decimal point** Determines the location of the decimal separator in the result string. 0.45678 ("0.00", en-US) -\> 0.46 0.45678 ("0.00", fr-FR) -\> 0,46 |  **"," Group separator and number scaling** Serves as both a group separator and a number scaling specifier. As a group separator, it inserts a localized group separator character between each group. As a number scaling specifier, it divides a number by 1000 for each comma specified. 2147483647 ("##,#", en-US) -\> 2,147,483,647 2147483647 ("##,#", es-ES) -\> 2.147.483.647 Scaling specifier: 2147483647 ("#,#,,", en-US) -\> 2,147 2147483647 ("#,#,,", es-ES) -\> 2.147 |
| --- | --- | --- | --- |
| **"%" Percentage placeholder** Multiplies a number by 100 and inserts a localized percentage symbol in the result string. 0.3697 ("%#0.00", en-US) -\> %36.97 0.3697 ("%#0.00", el-GR) -\> %36,97 0.3697 ("##.0 %", en-US) -\> 37.0 % 0.3697 ("##.0 %", el-GR) -\> 37,0 % | **"‰" Per mille placeholder** Multiplies a number by 1000 and inserts a localized per mille symbol in the result string. 0.03697 ("#0.00‰", en-US) -\> 36.97‰ 0.03697 ("#0.00‰", ru-RU) -\> 36,97‰ |  **"E0" "E+0" "E-0" "e0" "e+0" "e-0" Exponential notation** If followed by at least one 0 (zero), formats the result using exponential notation. The case of "E" or "e" indicates the case of the exponent symbol in the result string. The number of zeros following the "E" or "e" character determines the minimum number of digits in the exponent. A plus sign (+) indicates that a sign character always precedes the exponent. A minus sign (-) indicates that a sign character precedes only negative exponents. 987654 ("#0.0e0") -\> 98.8e4 1503.92311 ("0.0##e+00") -\> 1.504e+03 1.8901385E-16 ("0.0e+00") -\> 1.9e-16&nbsp; | **\\ Escape character** Causes the next character to be interpreted as a literal rather than as a custom format specifier. 987654 ("\\###00\\#") -\> #987654# |
|  **'string' "string"** Literal string delimiter Indicates that the enclosed characters should be copied to the result string unchanged. 68 ("# ' degrees'") -\> 68 degrees 68 ("#' degrees'") -\> 68 degrees |  **; Section separator** Defines sections with separate format strings for positive, negative, and zero numbers. 12.345 ("#0.0#;(#0.0#);-\\0-") -\> 12.35 0 ("#0.0#;(#0.0#);-\\0-") -\> -0- -12.345 ("#0.0#;(#0.0#);-\\0-") -\> (12.35) 12.345 ("#0.0#;(#0.0#)") -\> 12.35 0 ("#0.0#;(#0.0#)") -\> 0.0 -12.345 ("#0.0#;(#0.0#)") -\> (12.35) |  **Other** All other characters The character is copied to the result string unchanged. 68 ("# °") -\> 68 ° | &nbsp; |


### ASCII

#### Method1

Using formatting string {0:ASCII} its working with all types.

![Image](<lib/NewItem30.png>)


![Image](<lib/NewItem29.png>)


#### Method2

This setting show you how you can get ASCII code from remote device using formatting and VBScript.


**Internal Tags**

![Image](<lib/NewItem1.png>)![Image](<lib/NewItem2.png>)

**Tags Settings**

![Image](<lib/NewItem10.png>)![Image](<lib/NewItem11.png>)

**Display Data**

![Image](<lib/NewItem12.png>)

## Configuration Example

![Image](<lib/NewItem8.png>)

![Image](<lib/NewItem7.png>)

