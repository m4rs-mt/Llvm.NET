//===- IRBindings.h - Additional bindings for IR ----------------*- C++ -*-===//
//
//                     The LLVM Compiler Infrastructure
//
// This file is distributed under the University of Illinois Open Source
// License. See LICENSE.TXT for details.
//
//===----------------------------------------------------------------------===//
//
// This file defines additional C bindings for the IR component.
//
//===----------------------------------------------------------------------===//

#ifndef LLVM_BINDINGS_LLVM_IRBINDINGS_H
#define LLVM_BINDINGS_LLVM_IRBINDINGS_H

#include "llvm-c/Core.h"
#ifdef __cplusplus
#include "llvm/IR/Metadata.h"
#include "llvm/Support/CBindingWrapping.h"
#endif

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct LLVMOpaqueMetadata* LLVMMetadataRef;
typedef struct LLVMOpaqueMDOperand* LLVMMDOperandRef;


LLVMMetadataRef LLVMConstantAsMetadata(LLVMValueRef Val);
LLVMMetadataRef LLVMMDString2(LLVMContextRef C, const char *Str, unsigned SLen);
LLVMMetadataRef LLVMMDNode2(LLVMContextRef C, LLVMMetadataRef *MDs, unsigned Count);
LLVMMetadataRef LLVMTemporaryMDNode(LLVMContextRef C, LLVMMetadataRef *MDs, unsigned Count);

char const* LLVMGetMDStringText( LLVMMetadataRef mdstring, unsigned* len );

void LLVMAddNamedMetadataOperand2(LLVMModuleRef M, const char *name, LLVMMetadataRef Val);
void LLVMSetMetadata2(LLVMValueRef Inst, unsigned KindID, LLVMMetadataRef MD);
void LLVMMetadataReplaceAllUsesWith(LLVMMetadataRef MD, LLVMMetadataRef New);
void LLVMSetCurrentDebugLocation2(LLVMBuilderRef Bref, unsigned Line, unsigned Col, LLVMMetadataRef Scope, LLVMMetadataRef InlinedAt);

LLVMBool LLVMIsTemporary( LLVMMetadataRef M );
LLVMBool LLVMIsResolved( LLVMMetadataRef M );
LLVMBool LLVMIsUniqued( LLVMMetadataRef M );
LLVMBool LLVMIsDistinct( LLVMMetadataRef M );

void LLVMMDNodeResolveCycles( LLVMMetadataRef M );
char const* LLVMGetDIFileName( LLVMMetadataRef /*DIFile*/ file );
char const* LLVMGetDIFileDirectory( LLVMMetadataRef /*DIFile*/ file );

LLVMValueRef LLVMBuildAtomicCmpXchg( LLVMBuilderRef B, LLVMValueRef Ptr, LLVMValueRef Cmp, LLVMValueRef New, LLVMAtomicOrdering successOrdering, LLVMAtomicOrdering failureOrdering, LLVMBool singleThread );

LLVMMetadataRef LLVMFunctionGetSubprogram( LLVMValueRef function );
void LLVMFunctionSetSubprogram( LLVMValueRef function, LLVMMetadataRef subprogram );

#ifdef __cplusplus
}

namespace llvm {

DEFINE_ISA_CONVERSION_FUNCTIONS(Metadata, LLVMMetadataRef)

inline Metadata **unwrap(LLVMMetadataRef *Vals) {
  return reinterpret_cast<Metadata**>(Vals);
}

}

#endif

#endif
