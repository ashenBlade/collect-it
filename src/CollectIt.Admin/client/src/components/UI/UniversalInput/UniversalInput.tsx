export {}
// import React, {createElement} from 'react';
// import {RestrictionType} from "../../entities/restriction-type";
//
// const UniversalInput = (selectedItem: string) => {
//     let child:Array<React.ReactElement> = [];
//     switch (selectedItem as RestrictionType){
//         case RestrictionType.AllowAll:
//             break;
//         case RestrictionType.Author:
//             child.push(createElement('label', {}, 'Author: '))
//             child.push(createElement('input', {className: 'form-control',
//                 placeholder: 'Author'}))
//             break;
//         case RestrictionType.DaysAfter:
//             child.push(createElement('label', {}, 'Days After: '))
//             child.push(createElement('input', {className: 'form-control',
//                 placeholder: 'Days After', type: 'number'}))
//             break;
//         case RestrictionType.DaysTo:
//             child.push(createElement('label', {}, 'Days To: '))
//             child.push(createElement('input', {className: 'form-control',
//                 placeholder: 'Days To', type: 'number'}))
//             break;
//         case RestrictionType.DenyAll:
//             break;
//         case RestrictionType.Size:
//             child.push(createElement('label', {}, 'Size: '))
//             child.push(createElement('input', {className: 'form-control',
//                 placeholder: 'Size', type: 'number'}))
//             break;
//         case RestrictionType.Tags:
//             child.push(createElement('label', {}, 'Tags: '))
//             child.push(createElement('input', {className: 'form-control',
//                 placeholder: 'Tags separated by whitespaces'}))
//             break;
//     }
//     const element = createElement('div',
//         {id: selectedItem, className: 'row m-0 mb-3 mt-2'}, child);
//     return (
//         element
//     );
// };
//
// export default UniversalInput;