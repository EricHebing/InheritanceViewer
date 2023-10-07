#ifndef MY_CLASSES_H
#define MY_CLASSES_H

#include <string>

class BaseClass {
public:
    BaseClass();
    virtual ~BaseClass();

    void basePublicFunction();

private:
    int privateData;
};

class DerivedClass1 : public BaseClass {
public:
    DerivedClass1();
    virtual ~DerivedClass1();

    void derived1PublicFunction();

protected:
    double protectedData;
};

class DerivedClass2 : public BaseClass {
public:
    DerivedClass2();
    virtual ~DerivedClass2();

    void derived2PublicFunction();

private:
    std::string privateData;
};

class GrandChildClass : public DerivedClass1, protected DerivedClass2 {
public:
    GrandChildClass();
    virtual ~GrandChildClass();

    void grandChildPublicFunction();

private:
    char privateData;
};

class AnotherBaseClass {
public:
    AnotherBaseClass();
    virtual ~AnotherBaseClass();

    void anotherBasePublicFunction();

protected:
    bool protectedData;
};

class MultiDerivedClass : public BaseClass, public AnotherBaseClass {
public:
    MultiDerivedClass();
    virtual ~MultiDerivedClass();

    void multiDerivedPublicFunction();

private:
    float privateData;
};

class SingleInheritanceClass : public MultiDerivedClass {
public:
    SingleInheritanceClass();
    virtual ~SingleInheritanceClass();

    void singleInheritancePublicFunction();

private:
    int privateData;
};

class SimpleClass {
public:
    SimpleClass();
    virtual ~SimpleClass();

    void simplePublicFunction();

private:
    int privateData;
};

class MultipleInheritanceClass : public SimpleClass, public AnotherBaseClass {
public:
    MultipleInheritanceClass();
    virtual ~MultipleInheritanceClass();

    void multipleInheritancePublicFunction();

protected:
    double protectedData;
};

class FinalClass : public MultipleInheritanceClass {
public:
    FinalClass();
    virtual ~FinalClass();

    void finalPublicFunction();

private:
    std::string privateData;
};

#endif // MY_CLASSES_H