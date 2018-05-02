//---------------------------------------------------------------------------

#ifndef TxAttributeH
#define TxAttributeH
#include <vector>

template  <class T>
class BcTxAttribute {
protected:
    T value;
    T oldValue;
    bool initialized;
public:
    BcTxAttribute () : fetched(false) {};
    BcTxAttribute (const BcTxAttribute&);
    ~BcTxAttribute () {};

    T getValue();
    void setValue(const T& newVal);
    T& operator= (const T&);
    operator T ();
    T& operator () ();

    void init (const T& initVal);

    void apply();
    void discard();
};

template  <class T>
inline T BcTxAttribute<T>::getValue() { return value; }
template  <class T>
inline void BcTxAttribute<T>::setValue(const T& newVal) { value = newVal; }
template  <class T>
inline T& BcTxAttribute<T>::operator= (const T& newValue) { setValue(newValue); return value; };
template  <class T>
inline BcTxAttribute<T>::operator T () { return value; };
template  <class T>
inline T& BcTxAttribute<T>::operator () () { return value; };
template  <class T>
inline void BcTxAttribute<T>::init(const T& initVal) { value = oldValue = initVal; initialized = true; }
template  <class T>
inline void BcTxAttribute<T>::apply() { oldValue = value; }
template  <class T>
inline void BcTxAttribute<T>::discard() { value = oldValue; }

//---------------------------------------------------------------------------
class BcTxVectorAttribute : public BcTxAttribute<std::vector<double> > {
private:

protected:

public:

};

//---------------------------------------------------------------------------
#endif

