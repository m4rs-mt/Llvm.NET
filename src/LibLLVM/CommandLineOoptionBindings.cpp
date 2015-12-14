#include "llvm\Support\CommandLine.h"
#include "llvm\Support\Options.h"
#include "llvm-c\Support.h"

using namespace llvm;

namespace
{
    // template class to act as a holder of a custom command line options value for the C API for other language bindings
    template<typename T>
    struct GenericOptionHolder
    {
    private:
        T Value;

    public:
        static void RegisterOption( char const* name, char const* description, T const&  initialValue )
        {
            auto registry = OptionRegistry::instance( );
            registry.registerOption< T, GenericOptionHolder<T>, &GenericOptionHolder<T>::Value>( name, description, initialValue );
        }

        static T GetOptionValue( char const* name )
        {
            auto registry = OptionRegistry::instance( );
            return registry.get< T, GenericOptionHolder<T>, &GenericOptionHolder<T>::Value>( );
        }
    };
}

extern "C"
{
    void LLVMCreateCustomBooleanOption( char const* name, char const* description, LLVMBool initialValue )
    {
        GenericOptionHolder<bool>::RegisterOption( name, description, ( bool )initialValue );
    }

    void LLVMCreateCustomUnsignedOption( char const* name, char const* description, unsigned initialValue )
    {
        GenericOptionHolder<unsigned>::RegisterOption( name, description, initialValue );
    }

    void LLVMCreateCustomStringOption( char const* name, char const* description, char const* initialValue )
    {
        GenericOptionHolder<std::string>::RegisterOption( name, description, initialValue );
    }

    void LLVMCreateCustomDoubleOption( char const* name, char const* description, unsigned initialValue )
    {
        GenericOptionHolder<unsigned>::RegisterOption( name, description, initialValue );
    }

    LLVMBool LLVMGetBooleanOption( char const* name, /* out */ LLVMBool* value )
    {
        auto & optionsMap = cl::getRegisteredOptions( );
        auto pEntry = optionsMap.find( name );
        if( pEntry == optionsMap.end( ) )
            return false;
        
        cl::Option* pOpt = pEntry->second;

        if( typeid( *pOpt ) != typeid( cl::opt<bool> ) )
            return false;

        *value = static_cast< cl::opt<bool>* >( pOpt )->getValue( );
        return false;
    }

    LLVMBool LLVMGetUnsignedOption( char const* name, /* out */ unsigned* value )
    {
        auto & optionsMap = cl::getRegisteredOptions( );
        auto pEntry = optionsMap.find( name );
        if( pEntry == optionsMap.end( ) )
            return false;

        if( typeid( *( pEntry->second ) ) != typeid( cl::opt<unsigned> ) )
            return false;

        *value = static_cast< cl::opt<unsigned>* >( pEntry->second )->getValue( );
        return true;
    }

    char const* LLVMGetStringOption( char const* name )
    {
        auto & optionsMap = cl::getRegisteredOptions( );
        auto opt = optionsMap.find( name );
        if( opt == optionsMap.end( ) )
            return nullptr;

        if( typeid( *(opt->second ) ) != typeid( cl::opt<std::string> ) )
            return nullptr;

        return static_cast< cl::opt<std::string>* >( opt->second )->c_str( );
    }
}