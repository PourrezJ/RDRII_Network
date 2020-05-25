#pragma once

#pragma unmanaged
namespace rh2
{
    class Fiber
    {
      public:
        using StartRoutine = void(__cdecl*)(void* pParam);

      private:
        void* m_fiber = nullptr;

        Fiber(void* fiber) : m_fiber(fiber) {}

      public:
        Fiber() : m_fiber(nullptr) {}
        Fiber(const Fiber& fiber) : m_fiber(fiber.m_fiber) {}

        void switchTo() const;

        void remove() const;

        static Fiber CreateFiber(StartRoutine startAddress, void* pParam = nullptr);

        static Fiber CurrentFiber();

        static Fiber ConvertThreadToFiber();

        inline bool isNull()
        {
            return m_fiber == nullptr;
        }

        inline bool operator==(const Fiber& fiber)
        {
            return m_fiber == fiber.m_fiber;
        }

        inline bool operator!=(const Fiber& fiber)
        {
            return m_fiber != fiber.m_fiber;
        }

        operator bool()
        {
            return !isNull();
        }
    };
} // namespace rh2
